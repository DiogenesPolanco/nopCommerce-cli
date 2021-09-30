import {PluginsController} from "./plugin.js";
import 'regenerator-runtime/runtime'
import yargs from 'yargs-parser'

let {create} = new PluginsController();

describe('Plugins Controller tests', () => {
    test('create Widgets 4.30', () => {
        let args = yargs("-g=Widgets -p=Test");
        create(  args).then((message)=>{
            expect(args!== null).toBe(true)
        });
    });
});