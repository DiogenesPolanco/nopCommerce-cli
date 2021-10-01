import shell from 'shelljs'

export default class Helper {
    static printHandler(error, data, exit) {
        let obj = {error: error, data: data};
        if (obj.error) {
            if(obj.error.code === undefined){
                shell.echo(`${obj.error}\n`);
            }else{
                shell.echo(`${obj.error.code}: ${obj.error.message}\n`);
            }
        } else { 
            if(obj.data.message === undefined){
                shell.echo(`${obj.data}\n`);
            }else{
                shell.echo(`${obj.data.message}\n`);
            }
        }
        if(exit){
             shell.exit(exit);
        }
    }
}