import {Helper} from '../helper/index.js'
import {config} from '../config/index.js'
import {PluginService} from '../services/plugin.js'

let {build, create} = new PluginService();

export class PluginsController {

    create(yargs) {
        create(yargs, config.getPath())
            .then((data) => {
                Helper.printHandler(null, data)
            }).catch((error) => {
            Helper.printHandler(error, null)
        });
    }
    build(yargs) {
        build(yargs, config.getPath())
            .then((data) => {
                Helper.printHandler(null, data)
            }).catch((error) => {
            Helper.printHandler(error, null)
        });
    }
}