import {PluginsController} from "./plugin.js";
import {messages} from '../helper/messages.js'
import yargs from 'yargs'
let {create} = new PluginsController();

describe('Plugins Controller tests', () => {
    test('create Widgets 4.30', () => {
        create(  yargs. argv).then((message)=>{
            expect(message.code).toContain(messages["002"].code);
        });
    });
});