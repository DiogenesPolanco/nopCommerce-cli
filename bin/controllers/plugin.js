import {Helper} from '../helper/index.js'
import {config} from '../config/index.js'
import {PluginService} from '../services/plugin.js'

export class PluginsController {

    create(yargs) {
        new PluginService(). CreateAsync(yargs, config.getPath())
            .then((data) => {
                Helper.printHandler(null, data)
            }).catch((error) => {
            Helper.printHandler(error, null)
        });
    }
    build(yargs) {
        new PluginService(). Build(yargs, config.getPath())
            .then((data) => {
                Helper.printHandler(null, data)
            }).catch((error) => {
            Helper.printHandler(error, null)
        });
    }
}