import yargs from "yargs"
import Config from "./config/index.js"
import pluginsController from "./controllers/plugin.js"

yargs.usage("$0 command")
    .option('g', Config.getGroupAlias())
    .option('p', Config.getPluginAlias())
    .option('v', Config.getVersionAlias())
    .option('c', Config.getClearAlias())
    .option('b', Config.getBuildAlias())
    .option('i', Config.getInitAlias())
    .option('d', Config.getDotNetAlias())
    .option('git', Config.getGitAlias())
    .command("config", Config.getDescriptionConfigCommand(), yargs => pluginsController.config(yargs))
    .command("new", Config.getDescriptionNewCommand(), yargs => pluginsController.create(yargs))
    .command("build", Config.getDescriptionBuildCommand(), yargs => pluginsController.build(yargs))
    .command("init", Config.getDescriptionInitCommand(), yargs => pluginsController.init(yargs))
    .demand(1, Config.getDescriptionDemand())
    .showHelpOnFail(true)
    .help("h")
    .alias("h", "help")
    .argv
