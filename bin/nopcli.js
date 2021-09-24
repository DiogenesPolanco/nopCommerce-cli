#! /usr/bin/env node

var shell = require("shelljs");
var yargs = require("yargs");

var argv = yargs.usage("$0 command")
  .command("new", "create plugin", function (yargs) {
    shell.echo('this new command is not available');
  })
  .command("build", "build plugin", function (yargs) {
    shell.echo('this build command is not available');
  })
  .command("deploy", "deploy plugin", function (yargs) {
    shell.echo('this deploy command is not available');
  })
  .demand(1, "must provide a valid command")
  .help("h")
  .alias("h", "help")
  .argv
