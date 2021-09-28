#! /usr/bin/env node

import yargs from 'yargs'
import {config} from './config/index.js'
import {PluginsController} from './controllers/plugin.js'
let {build, create} = new PluginsController();

var argv = yargs.usage("$0 command")
    .option('g', config.getGroupAlias())
    .option('p', config.getPluginAlias())
    .option('v', config.getVersionAlias())
    .command("new", config.getDescriptionNewCommand(), yargs => create(yargs))
    .command("build", config.getDescriptionBuildCommand(), yargs => build(yargs))
    .demand(1, config.getDescriptionDemand())
    .showHelpOnFail(true)
    .help("h")
    .alias("h", "help")
    .argv
