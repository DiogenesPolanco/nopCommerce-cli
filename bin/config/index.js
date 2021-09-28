import  appRoot from 'app-root-path'

export class config {
    static getPath() {
        return appRoot.path;
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

    static getDescriptionNewCommand() {
        return "create plugin -[g] -[p] -[v]";
    }

    static getDescriptionBuildCommand() {
        return "build plugin -[g] -[p]";
    }

    static getDescriptionDemand() {
        return "please choose a valid command";
    }
}
