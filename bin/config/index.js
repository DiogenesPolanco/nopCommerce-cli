import {getInstalledPath} from 'get-installed-path'

export default class Config {
    static getPath() {
        return new Promise((resolve) => {
            getInstalledPath('nopcli').then((path) => {
                resolve(path !== undefined ? path : "./");
            }).catch(() => {
                resolve("./");
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
            default: 430,
            describe: 'Only support ["4.20", "4.30", "4.40", "4.50"]'
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
}