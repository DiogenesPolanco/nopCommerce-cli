import Validator from 'validatorjs';
import pluginService from '../services/plugin.js'
import {messages} from '../helper/messages.js'
import fs from "fs";
import shell from "shelljs";

class ValidatorService {
    getResponse(val) {
        let errors = val.errors.all();
        return {
            valid: val.passes(),
            hasErrors: errors.length > 0,
            errors: errors
        }
    }

    validateCreatePlugin(args) {
        let self = this;
        return new Promise((resolve) => {
            let rules = {
                g: 'required',
                p: 'required',
                c: 'required|boolean',
                b: 'required|boolean',
                exitPlugin: 'required|boolean|hasClearPlugin|hasBuildPlugin'
            };
            args.exitPlugin = fs.existsSync(pluginService.getOutProjectPathPluginName(args));
            let val = new Validator(args, rules);

            Validator.register('hasBuildPlugin', function (value) {
                return (value && args.b ) ;
            }, pluginService.ReplacePluginName(messages["001-1"].message, args));

            Validator.register('hasClearPlugin', function (value) {
                return (value && args.c ) || !value;
            }, pluginService.ReplacePluginName(messages["001"].message, args));

            resolve(self.getResponse(val));
        });
    }

    validateBuildPlugin(args) {
        let self = this;
        return new Promise((resolve) => {
            let rules = {
                g: 'required',
                p: 'required',
                exitPlugin: 'required|boolean'
            };
            args.exitPlugin = fs.existsSync(pluginService.getOutProjectPathPluginName(args));
            let val = new Validator(args, rules);
            resolve(self.getResponse(val));
        });
    }

    validateClonePlugin(args) {
        let self = this;
        return new Promise((resolve) => {
            let rules = {
                exitGit: 'boolean|hasGit'
            };
            args.exitGit = shell.which('git') !== null;
            let val = new Validator(args, rules);

            Validator.register('hasGit', function (value) {
                return value;
            }, pluginService.ReplacePluginName(messages["000"].message, args));
            resolve(self.getResponse(val));
        });
    }
}

const validatorService = new ValidatorService();
export default validatorService;