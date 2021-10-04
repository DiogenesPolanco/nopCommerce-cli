import shell from 'shelljs'
import fs from 'fs'
import {messages} from '../helper/messages.js'
import Helper from '../helper/index.js'
import ProgressService from "./progress.js";
import Config from '../config/index.js'


class PluginService {

    getSrcPluginName(args) {
        return `Nop.Plugin.${args.g}.NopCliGeneric`;
    }

    getOutPluginName(args) {
        return `Nop.Plugin.${args.g}.${args.p}`;
    }

    getFullSrcPlugin(args) {
        let self = this;
        return `${self.getSrcSolutionPath()}/Plugins/${self.getOutPluginName(args)}`;
    }

    getSrcSolutionPath() {
        return fs.existsSync(`Plugins`) ? `.` : `src`;
    }

    validateVersion(version) {
        let self = this;
        let slPath = self.getSrcSolutionPath();
        if (fs.existsSync(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`)) {
            fs.readFile(`${slPath}/Libraries/Nop.Services/Plugins/Samples/uploadedItems.json`, 'utf8', (err, data) => {
                if (err) throw err;
                let nopVersionFile = JSON.parse(data.replace(new RegExp("\/\/(.*)", "g"), ''));
                version = nopVersionFile[3].SupportedVersion;
            });
        }
        return version;
    }

    getSrcVersion(args) {
        let self = this;
        let result = args.v !== undefined && args.v !== 420 ? args.v === 450 ? 440 : args.v : 430;
        return self.validateVersion(result);
    }

    getOutProjectPathPluginName(args) {
        let self = this;
        return `${self.getFullSrcPlugin(args)}/${self.getOutPluginName(args)}.csproj`;
    }

    existOutProject(args) {
        let self = this;
        return new Promise((resolve, reject) => {
            let path = self.getOutProjectPathPluginName(args);
            if (path === undefined) {
                reject(messages['005']);
            } else {
                resolve(fs.existsSync(path));
            }
        });
    }

    copyFiles(root_path, args) {
        let self = this;
        return new Promise((resolve) => {
            let pluginsPath = self.getFullSrcPlugin(args);
            let srcPluginName = self.getSrcPluginName(args);
            let pluginName = self.getOutPluginName(args);

            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${root_path}/src/nopCommerce-${self.getSrcVersion(args)}/${srcPluginName}/`, pluginsPath);
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            fs.readFileSync(`${root_path}/src/assets/images/logos/logo.png`, function (err, data) {
                if (err) {
                    resolve(false);
                } else {
                    fs.writeFileSync(`${pluginsPath}/logo.png`, data, 'base64');
                }
            });
            resolve(true);
        });
    }

    replaceContentFiles(args) {
        let self = this;
        return new Promise((resolve) => {
            let pluginsPath = self.getFullSrcPlugin(args);
            let files = shell.find(`${pluginsPath}`);
            if (files.length > 0) {
                for (const fileOrFolder of files) {
                    fs.lstat(fileOrFolder, (err, stats) => {
                        if (stats.isFile()) {
                            let fileName = fileOrFolder.replace("NopCliGeneric", args.p);
                            shell.sed('-i', /NopCliGeneric/g, args.p, fileOrFolder);
                            if (fileName !== fileOrFolder) {
                                shell.mv(`${fileOrFolder}`, `${fileName}`);
                            }
                        }
                    });
                }
            }
            resolve(files.length > 0);
        });
    }

    addSolution(args) {
        let self = this;
        return new Promise((resolve, reject) => {
            if (fs.existsSync(`${self.getSrcSolutionPath()}/NopCommerce.sln`)) {
                Helper.printHandler(null, messages["006"].message.replace('{{nopCli}}', self.getOutPluginName(args)))
                shell.config.silent = true;
                ProgressService.waitProgress().then(() => {
                    shell.cd(self.getSrcSolutionPath());
                    shell.exec(`dotnet sln add ./Plugins/${self.getOutPluginName(args)}`);
                    resolve(messages["002"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                });
            } else {
                reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
            }
        });
    }

    clearPlugin(args) {
        let self = this;
        return new Promise((resolve, reject) => {
            self.existOutProject(args).then((result) => {
                if (result) {
                    shell.rm("-r", self.getFullSrcPlugin(args));
                }
                resolve(result);
            }).catch((error) => {
                reject(error);
            });
        });
    }

    createProject(args, root_path) {
        let self = this;
        return new Promise((resolve, reject) => {
            self.copyFiles(root_path, args).then((copied) => {

                if (copied) {
                    self.replaceContentFiles(args).then((success) => {
                        if (success) {
                            self.addSolution(args).then((message) => {
                                resolve(message);
                            }).catch((error) => {
                                reject(error);
                            });
                        } else {
                            reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                        }
                    }).catch((error) => {
                        reject(error);
                    });
                } else {
                    reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }

    TryToCreate(args, root_path) {
        let self = this;
        return new Promise((resolve, reject) => {
            self.createProject(args, root_path).then((messages) => {
                resolve(messages);
            }).catch(() => {
                self.clearPlugin(args).then((result) => {
                    resolve(messages[result ? "002" : "001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                }).catch((error) => {
                    reject(error);
                });
            });
        });
    }

    Create(args, root_path) {
        let self = this;
        return new Promise((resolve, reject) => {

            self.existOutProject(args).then((existProject) => {

                if (existProject === false) {
                    self.TryToCreate(args, root_path).then((messages) => {
                        resolve(messages)
                    }).catch((error) => {
                        reject(error);
                    });
                } else {
                    if (args.c) {
                        self.clearPlugin(args).then((result) => {
                            self.TryToCreate(args, root_path, result)
                                .then((messages) => {
                                    resolve(messages)
                                }).catch((error) => {
                                reject(error);
                            });
                        })
                    } else {
                        reject(messages["001"].message.replace('{{nopCli}}', self.getOutPluginName(args)));
                    }
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }

    clone(args) {
        return new Promise((resolve, reject) => {
            if (!shell.which('git')) {
                resolve('Sorry, this script requires git');
                shell.exit(1);
            } else {
                ProgressService.waitInfinityProgress((progress) => {
                    shell.exec(Config.getCloneNopDefaultCommand(), function (code, stdout, stderr) {
                        ProgressService.SetCompleted(progress, () => {
                            if (stderr) {
                                resolve(messages['009']);
                            } else if (args.git) {
                                shell.rm("-r", Config.getGitNopCommercePath());
                                shell.exec("git init && git add *.*", function (codeGit, stdoutGit, stderrGit) {
                                    if (stderrGit) {
                                        reject(stderrGit)
                                    } else {
                                        resolve(messages['008']);
                                    }
                                });
                            } else {
                                resolve(messages['008']);
                            }
                        });
                    });
                });
            }
        });
    }

    Build(args) {
        let self = this;
        return new Promise((resolve, reject) => {
            self.existOutProject(args).then((existProject) => {
                if (existProject) {
                    shell.cd(self.getSrcPluginName(args));
                    shell.exec(`dotnet build ${self.getOutPluginName(args)}.csproj`);
                    resolve(messages['003']);
                } else {
                    reject(messages['004']);
                }
            });
        });
    }

    Init(args, root_path) {
        let self = this;
        return new Promise((resolve, reject) => {
            self.clone(args).then((msg) => {
                if (args.p) {
                    self.createProject(args, root_path).then((msgCreated) => {
                        if (args.b) {
                            self.Build(args).then((msgBuild) => {
                                resolve(msgBuild);
                            });
                        } else {
                            resolve(msgCreated);
                        }
                    })
                } else {
                    resolve(msg);
                }
            }).catch((error) => {
                reject(error);
            });
        });
    }
}

const pluginService = new PluginService();
export default pluginService;