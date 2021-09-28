import {Helper} from '../helper/index.js'
import {config} from '../config/index.js'
import {PluginService} from '../services/plugin.js'

export class PluginsController {

    async create(yargs) {
        await new PluginService().CreateAsync(yargs, config.getPath())
            .then((data) => {
                Helper.printHandler(null, data)
            }).catch((error) => {
                Helper.printHandler(error, null)
            });
    }
    async build(yargs) {
        await new PluginService().Build(yargs, config.getPath())
            .then((data) => {
                Helper.printHandler(null, data)
            }).catch((error) => {
                Helper.printHandler(error, null)
            });
    }
}