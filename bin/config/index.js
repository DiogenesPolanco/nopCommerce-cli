export class config {
    static getPath() {
        return process.env.NODE_ENV === 'Development' ? "./" : `${process.mainModule.paths[2]}/nopcli`;
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
            type: 'string',
            default: '430',
            describe: 'Only support ["4.20", "4.30", "4.40"]'
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
