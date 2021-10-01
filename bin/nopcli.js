import yargs from "yargs"
import {config} from "./config/index.js"
import {PluginsController} from "./controllers/plugin.js"

let {build, create, init} = new PluginsController();

yargs.usage("$0 command")
    .option('g', config.getGroupAlias())
    .option('p', config.getPluginAlias())
    .option('v', config.getVersionAlias())
    .option('c', config.getClearAlias())
    .option('b', config.getBuildAlias())
    .option('i', config.getInitAlias())
    .command("new", config.getDescriptionNewCommand(), yargs => create(yargs))
    .command("build", config.getDescriptionBuildCommand(), yargs => build(yargs))
    .command("init", config.getDescriptionInitCommand(), yargs => init(yargs))
    .demand(1, config.getDescriptionDemand())
    .showHelpOnFail(true)
    .help("h")
    .alias("h", "help")
    .argv
