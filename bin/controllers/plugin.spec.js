import pluginsController from "./plugin.js";
import 'regenerator-runtime/runtime'
import yargs from 'yargs-parser'


describe('Plugins Controller tests', () => {
    it('create Widgets 4.30', (done) => {
        let args = yargs("-g=Widgets -p=Test");
        pluginsController.create(  { argv: args }).then(()=>{
            done()
        });
    });
    it('build Widgets 4.30', (done) => {
        let args = yargs("-g=Widgets -p=Test -c=true");
        pluginsController.build(  { argv: args }).then(()=>{
            done()
        });
    });
});