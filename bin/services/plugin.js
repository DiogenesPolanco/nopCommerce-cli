import shell from 'shelljs'
import fs from 'fs'
import {messages} from '../helper/messages.js'
import Helper from '../helper/index.js'
import ProgressService from "./progress.js";
import Config from '../config/index.js'


class PluginService {

    ReplacePluginName(message, args) {
        let self = this;
        return message.replace('{{nopCli}}', self.getOutPluginName(args));
    }

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
        return self.validateVersion(args.v);
    }

    getOutProjectPathPluginName(args) {
        let self = this;
        return `${self.getFullSrcPlugin(args)}/${self.getOutPluginName(args)}.csproj`;
    }

    existOutProject(args) {
        let self = this;
        return new Promise((resolve, reject) => {
            if (args.g === undefined && args.p === undefined) {
                resolve(false);
            } else {
                let path = self.getOutProjectPathPluginName(args);
                if (path === undefined) {
                    shell.echo(messages['005'])
                    reject(false);
                } else {
                    resolve(fs.existsSync(path));
                }
            }
        });
    }

    copyFiles(root_path, args) {
        let self = this;
        return new Promise(async (resolve) => {
            let pluginsPath = self.getFullSrcPlugin(args);
            let srcPluginName = self.getSrcPluginName(args);
            let pluginName = self.getOutPluginName(args);
            shell.mkdir('-p', `${pluginsPath}`);
            shell.cp('-R', `${root_path}/src/nopCommerce-${self.getSrcVersion(args)}/${srcPluginName}/*`, pluginsPath);
            shell.mv(`${pluginsPath}/${srcPluginName}.csproj`, `${pluginsPath}/${pluginName}.csproj`);

            fs.readFile(`${root_path}/src/assets/images/logos/logo.png`, function (err, data) {
                if (err) {
                    console.log(err);
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
                shell.echo("");
                Helper.printHandler(self.ReplacePluginName(messages["006"].message, args));
                ProgressService.waitProgress().then(() => {
                    shell.cd(self.getSrcSolutionPath());
                    shell.exec(`dotnet sln add ./Plugins/${self.getOutPluginName(args)}`);
                    resolve(self.ReplacePluginName(messages["002"].message, args));
                });
            } else {
                reject(self.ReplacePluginName(messages["003"].message, args));
            }
        });
    }

    clearPlugin(args) {
        let self = this;
        return new Promise((resolve) => {
            let result = args.c;
            if (result) {
                shell.rm("-r", self.getFullSrcPlugin(args));
            }
            resolve(result);
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
                            })
                        } else {
                            reject(self.ReplacePluginName(messages["001"].message, args));
                        }
                    })
                } else {
                    reject(self.ReplacePluginName(messages["001"].message, args));
                }
            });
        });
    }

    clone(args) {
        let self = this;
        return new Promise((resolve) => {
            if (args.git) {
                ProgressService.waitInfinityProgress((progress) => {
                    shell.exec(Config.getCloneNopDefaultCommand(), function () {
                        ProgressService.SetCompleted(progress, () => {
                            shell.rm("-r", Config.getGitNopCommercePath());
                            shell.exec("git init && git add *.*");
                            resolve(self.ReplacePluginName(messages["008"].message, args));
                        });
                    });
                });
            } else {
                resolve(self.ReplacePluginName(messages["008"].message, args));
            }
        });

    }

    Build(args) {
        let self = this;
        return new Promise((resolve) => {
            let result = args.b;
            if (result) {
                shell.cd(self.getFullSrcPlugin(args));
                shell.exec(`dotnet build ${self.getOutPluginName(args)}.csproj`);
                resolve(self.ReplacePluginName(messages["004"].message, args));
            } else {
                resolve(false);
            }
        });
    }

    Init(args) {
        let self = this;
        return new Promise((resolve, reject) => {
            self.clone(args).then((msg) => {
                resolve(msg);
            }).catch((error) => {
                reject(error);
            });
        });
    }
}

const pluginService = new PluginService();
export default pluginService;