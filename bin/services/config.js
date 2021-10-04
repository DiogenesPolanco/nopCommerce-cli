import fs from 'fs'
import {messages} from '../helper/messages.js'
import ProgressService from "./progress.js";
import Config from '../config/index.js'
import inquirer from "inquirer";

class ConfigService {
    CreateSetting(answers, reject, resolve) {
        fs.writeFile(Config.getClientSettingFileName(),
            JSON.stringify(answers, null, '  '), {
                encoding: 'utf8',
                flag: 'w'
            },
            (error) => {
                if (error) {
                    reject(error);
                } else {
                    resolve(messages['007']);
                }
            });
    }

    async Init() {
        let self = this;
        return new Promise(async (resolve, reject) => {
            inquirer
                .prompt(Config.getConfigPluginQuestions())
                .then((answers) => {
                    ProgressService.waitInfinityProgress((progress) => {
                        ProgressService.SetCompleted(progress, () => {
                            self.CreateSetting(answers, resolve, reject);
                        });
                    });
                });
        });
    }
}

const configService = new ConfigService();
export default configService;