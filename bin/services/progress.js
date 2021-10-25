import cliProgress from 'cli-progress'
import shell from 'shelljs';

export default class ProgressService {

    static waitProgress(type = cliProgress.Presets.shades_classic, value = 0, total = 100, ms = 10) {

        let self = this;
        let progress = new cliProgress.Bar({
            format: 'progress [{bar}] {percentage}%'
        }, type);
        progress.start(total, 0);

        return new Promise((resolve) => {

            const timer = setInterval(() => {
                // increment value
                value++;
                // update the bar value
                progress.update(value)

                // set limit
                if (value >= progress.getTotal()) {
                    // stop timer
                    clearInterval(timer);

                    progress.stop();

                    // run complete callback
                    shell.echo("");
                    resolve(self);
                }
            }, ms);
        });
    }

    static waitInfinityProgress(callback, silent=true, clearOnComplete=true,  type = cliProgress.Presets.rect, value = 0, total = 100, ms = 500) {

         shell.config.silent = silent;

        let progress = new cliProgress.Bar({
            clearOnComplete:clearOnComplete,
            format: 'progress [{bar}]'
        }, cliProgress.Presets.rect);
        progress.start(total, value);

        callback(progress);

        const timer = setInterval(function () {
            // increment value
            value++;

            // update the bar value
            progress.update(value)

            // change the total value
            if (value === total) {
                total = total * 2;
                progress.setTotal(total);
            }

            // limit reached ?
            if (value >= progress.getTotal()) {
                // stop timer
                clearInterval(timer);
            }
        }, ms);
    }

    static  SetCompleted(progress, callback){
        progress.setTotal(100);
        progress.update(100);
        progress.stop();
        callback();
    }
}