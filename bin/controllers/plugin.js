import Helper from '../helper/index.js'
import Config from '../config/index.js'
import pluginService from '../services/plugin.js'
import shell from "shelljs";

export default class PluginsController {

    static async create(yargs) {
        return Config.getPath().then(async (path) => {
            await pluginService.CreateAsync(yargs, path)
                .then((data) => {
                    Helper.printHandler(null, data)
                }).catch((error) => {
                    Helper.printHandler(error, null)
                });

        });
    }

    static async build(yargs) {
        return Config.getPath().then(async (path) => {
            await pluginService.Build(yargs, path)
                .then((data) => {
                    Helper.printHandler(null, data)
                }).catch((error) => {
                    Helper.printHandler(error, null)
                });
        })
    }

    static async init(yargs) {
        return Config.getPath().then(async (path) => {
            await pluginService.Init(yargs, path)
                .then((data) => {
                    Helper.printHandler(null, data, true)

                }).catch((error) => {
                    Helper.printHandler(error, null, true)
                });
        });

    }
}