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
                c: 'required|boolean|hasClearPlugin',
                b: 'required|boolean|hasBuildPlugin',
                exitPlugin: 'required|boolean|exitPlugin'
            };
            args.exitPlugin = fs.existsSync(pluginService.getOutProjectPathPluginName(args));
            let val = new Validator(args, rules);

            Validator.register('exitPlugin', function (value) {
                return !value || args.c;
            }, pluginService.ReplacePluginName(messages["001"].message, args));

            Validator.register('hasClearPlugin', function (value) {
                let result = args.exitPlugin  && value;
                return  result ||  !result ;
            }, pluginService.ReplacePluginName(messages["001-2"].message, args));

            Validator.register('hasBuildPlugin', function (value) {
                let result = args.exitPlugin  && value;
                return  result ||  !result ;
            }, pluginService.ReplacePluginName(messages["001-3"].message, args));

            resolve(self.getResponse(val));
        });
    }

    validateBuildPlugin(args) {
        let self = this;
        return new Promise((resolve) => {
            let rules = {
                g: 'required',
                p: 'required',
                exitPlugin: 'required|boolean|hasBuildPlugin'
            };
            args.exitPlugin = fs.existsSync(pluginService.getOutProjectPathPluginName(args));
            let val = new Validator(args, rules);

            Validator.register('hasBuildPlugin', function (value) {
                return args.exitPlugin && value;
            }, pluginService.ReplacePluginName(messages["001-3"].message, args));

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