import Helper from '../helper/index.js'
import Config from '../config/index.js'
import pluginService from '../services/plugin.js'
import ConfigService from '../services/config.js'
import validatorService from '../services/validator.js'
import shell from "shelljs";

export default class PluginsController {

    static create(yargs) {
        shell.config.silent = false;
        Config.getPath().then((path) => {
            let args = Config.Prepare(path, yargs.argv)
            return validatorService.validateCreatePlugin(args).then((result) => {
                if (result.valid === false) {
                    shell.echo(result.errors)
                } else {
                    pluginService.clearPlugin(args, path).then(() => {
                        pluginService.createProject(args, path).then((message) => {
                            pluginService.Build(args, path).then(() => {
                                shell.echo(message)
                            }).catch((error) => {
                                Helper.printHandler(error, null)
                            });
                        })
                    })
                }
            });
        });
    }

    static build(yargs) {
        Config.getPath().then((path) => {
            let args = Config.Prepare(path, yargs.argv)
            return validatorService.validateBuildPlugin(args).then((result) => {
                if (result.valid === false) {
                    shell.echo(result.errors)
                } else {
                    pluginService.Build(args, path).then((message) => {
                        shell.echo(message)
                    }).catch((error) => {
                        Helper.printHandler(error, null)
                    });
                }
            });
        });
    }

    static init(yargs) {
        Config.getPath().then((path) => {
            let args = Config.Prepare(path, yargs.argv)
            return validatorService.validateClonePlugin(args).then((result) => {
                if (result.valid === false) {
                    shell.echo(result.errors)
                } else {
                    pluginService.clone(args).then((message) => {
                        shell.echo(message)
                    }).catch((error) => {
                        Helper.printHandler(error, null)
                    });
                }
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