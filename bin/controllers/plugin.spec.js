import {PluginsController} from "./plugin.js";
import 'regenerator-runtime/runtime'
import yargs from 'yargs-parser'

let {create} = new PluginsController();

describe('Plugins Controller tests', () => {
    it('create Widgets 4.30', (done) => {
        let args = yargs("-g=Widgets -p=Test"); 
        create(  { argv: args }).then(()=>{
            done()
        });
    });
});