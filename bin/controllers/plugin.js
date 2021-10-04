import Helper from '../helper/index.js'
import Config from '../config/index.js'
import pluginService from '../services/plugin.js'
import ConfigService from '../services/config.js'

export default class PluginsController {

    static create(yargs) {
        return Config.getPath().then((path) => {
            pluginService.Create(Config.Prepare(path, yargs.argv), path)
                .then((data) => {
                    Helper.printHandler(null, data)
                }).catch((error) => {
                Helper.printHandler(error, null)
            });

        });
    }

    static build(yargs) {
        return Config.getPath().then((path) => {
            pluginService.Build(Config.Prepare(path, yargs.argv), path)
                .then((data) => {
                    Helper.printHandler(null, data)
                }).catch((error) => {
                Helper.printHandler(error, null)
            });
        })
    }

    static init(yargs) {
        return Config.getPath().then((path) => {
            pluginService.Init(Config.Prepare(path, yargs.argv), path)
                .then((data) => {
                    Helper.printHandler(null, data, true)
                }).catch((error) => {
                Helper.printHandler(error, null, true)
            });
        });

    }

    static config(yargs) {
        return Config.getPath().then((path) => {
            ConfigService.Init(Config.Prepare(path, yargs.argv))
                .then((data) => {
                    Helper.printHandler(null, data, true)
                }).catch((error) => {
                Helper.printHandler(error, null, true)
            });
        });
    }
}