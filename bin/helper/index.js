import shell from 'shelljs'

export class Helper {
    static printHandler(error, data) {
        let obj = {error: error, data: data};
        if (obj.error) {
            shell.echo(obj.error);
        } else {
            shell.echo(obj.data);
        }
    }
}