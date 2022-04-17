import {getInstalledPath} from 'get-installed-path'
import inquirer from "inquirer";
import fs from 'fs'

export default class Config {
    static getPath(name, opts) {
        return new Promise((resolve) => {
            getInstalledPath('nopcli', opts).then((path) => {
                resolve(path === undefined ? path : ".");
            }).catch(() => {
                resolve(".");
            });
        });
    };

    static getGroupAlias() {
        return {
            alias: 'group',
            type: 'string',
            default: 'Widgets',
            describe: 'support Widgets, Payments, DiscountRules, Shipping and Misc'
        };
    }

    static getVersionAlias() {
        return {
            alias: 'version',
            type: 'number',
            default: 450,
            describe: 'Only support ["4.20", "4.30", "4.40", "4.50", "4.60"]'
        };
    }

    static getPluginAlias() {
        return {
            alias: 'plugin',
            type: 'string'
        };
    }

    static getClearAlias() {
        return {
            alias: 'clear',
            type: 'boolean',
            default: false
        };
    }

    static getBuildAlias() {
        return {
            alias: 'build',
            type: 'boolean',
            default: false
        };
    }

    static getInitAlias() {
        return {
            alias: 'init',
            type: 'boolean',
            default: false
        };
    }

    static getDescriptionConfigCommand() {
        return "config plugin -[g] -[p] -[v] -[c] -[b]";
    }

    static getDescriptionNewCommand() {
        return "create plugin -[g] -[p] -[v] -[c] -[b]";
    }

    static getDescriptionBuildCommand() {
        return "build plugin -[g] -[p]";
    }

    static getDescriptionInitCommand() {
        return "init plugin -[g] -[p] -[v] -[b]";
    }

    static getDescriptionDemand() {
        return "please choose a valid command";
    }

    static getCloneNopDefaultCommand() {
        return "git clone https://github.com/nopSolutions/nopCommerce.git ./ --branch release-4.30 --depth 1";
    }

    static getGitNopCommercePath() {
        return "./.git";
    }

    static getDotNetAlias() {
        return {
            alias: 'dotnet',
            type: 'boolean',
            default: false
        };
    }

    static getGitAlias() {
        return {
            alias: 'git',
            type: 'boolean',
            default: false
        };
    }

    static getClientSettingFileName() {
        return '.nopcli';
    }

    static getConfigPluginQuestions() {
        return [
            {
                type: 'list',
                name: 'group',
                message: 'What plugin group do you want to have by default?',
                choices: [
                    'DiscountRoles',
                    'Misc',
                    'Payments',
                    'Shipping',
                    'Widgets',
                    new inquirer.Separator(),
                    'Ask for help',
                    {
                        name: 'Contact support',
                        disabled: 'Unavailable at this time',
                    },
                    'Leave',
                ],
            },
            {
                type: 'list',
                name: 'version',
                message: 'What version of nopCommerce do you need?',
                choices: ['4.20', '4.30', '4.40', '4.50', '4.60'],
                filter(val) {
                    return val.replace(".", "");
                },
            },
            {
                type: 'input',
                name: 'name',
                message: 'What name do you want to use by default?',
                filter(val) {
                    return val.toLowerCase();
                },
                validate(value) {
                    const letters = value.match(/[a-zA-Z]+/g);
                    if (value !== "" && letters) {
                        return true;
                    }

                    return 'String- Please enter a default name for your plugins';
                }
            },
            {
                type: 'input',
                name: 'author',
                message: 'What author do you want to use by default?',
                default: "NopCli team",
                validate(value) {
                    const letters = value.match(/[a-zA-Z]+/g);
                    if (value !== "" && letters) {
                        return true;
                    }

                    return 'String- Please enter a default author for your plugins';
                }
            },
            {
                type: 'confirm',
                name: 'toBeAutoClear',
                message: 'Do you want to remove your plugins if they exist when trying to create?',
                default: false,
            },
            {
                type: 'confirm',
                name: 'toBeAutoBuild',
                message: 'Do you want to build after creating the plugins?',
                default: false,
            },
            {
                type: 'confirm',
                name: 'toBeIniCore',
                message: 'do you want to install dotnet core?',
                default: false,
            },
            {
                type: 'confirm',
                name: 'toBeIniNop',
                message: 'do you want to clone nopcommerce?',
                default: false,
            },
            {
                type: 'confirm',
                name: 'toBeIniGit',
                message: 'do you want to init git?',
                default: false,
            }
        ];
    }

    static   getSetting(path) {
        let self = this;

        if (fs.existsSync(`${path}/${self.getClientSettingFileName()}`)) {
            path = `${path}/${self.getClientSettingFileName()}`;
        } else if (fs.existsSync(`${path}/src/${self.getClientSettingFileName()}`)) {
            path = `${path}/src/${self.getClientSettingFileName()}`
        } else {
            return {};
        }
        let rawData =   fs.readFileSync(path);
        return JSON.parse(rawData);
    }

    static Prepare(path, args) {
        let self = this;

        let setting = self.getSetting(path);
        args.p = args.p === undefined ? setting.name : args.p;
        args.g = args.g === undefined ?  setting.group : args.g;
        args.v = args.v=== undefined ?  setting.version : args.v;
        args.a = args.a=== undefined ? setting.author : args.a;
        args.c = args.c === undefined ? setting.toBeAutoClear : args.c;
        args.b = args.b=== undefined ? setting.toBeAutoBuild : args.b;
        args.i = args.i === undefined ? setting.toBeIniNop : args.i;
        args.git = args.git === undefined ? setting.toBeIniGit : args.git;
        return args;
    }
}