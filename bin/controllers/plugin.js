import {Helper} from '../helper/index.js'
import {config} from '../config/index.js'
import {PluginService} from '../services/plugin.js'

export class PluginsController {

    async create(yargs) {
        config.getPath().then(async (path) => {
            await new PluginService().CreateAsync(yargs, path)
                .then((data) => {
                    Helper.printHandler(null, data)
                }).catch((error) => {
                    Helper.printHandler(error, null)
                });
        })
    }

    async build(yargs) {
        config.getPath().then(async (path) => {
            await new PluginService().Build(yargs, path)
                .then((data) => {
                    Helper.printHandler(null, data)
                }).catch((error) => {
                    Helper.printHandler(error, null)
                });
        })
    }
}