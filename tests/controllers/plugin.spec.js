import {PluginsController} from "../../bin/controllers/plugin.js";
import {messages} from '../../bin/helper/messages.js'
let {build, create} = new PluginsController();

describe('Plugins Controller tests', () => {
    test('create Widgets 4.30', () => {
        create(new {
            argv:{
                g:"Widgets",
                p:"test1",
                v:"430"
            }
        }).then((message)=>{
            expect(message.code).toContain(messages["002"].code);
        });
    });
});