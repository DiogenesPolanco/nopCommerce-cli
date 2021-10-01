import cliProgress from 'cli-progress'
import shell from 'shelljs';

export class ProgressService {

    static waitProgress(type = cliProgress.Presets.shades_classic, value = 0, total = 100, ms = 10) {

        let self = this;
        let progress = new cliProgress.Bar({
            format: 'progress [{bar}] {percentage}% | {value}/{total}'
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

    static waitProgressTwo(progress, value = 0, type = cliProgress.Presets.shades_classic, total = 100) {

        progress = progress ?? new cliProgress.Bar({
            format: 'progress [{bar}] {percentage}% | {value}/{total}'
        }, type);
        progress.start(total, 0);


        // increment value
        value++;
        // update the bar value
        progress.update(value)

        // set limit
        if (value >= progress.getTotal()) {

            progress.stop();

            // run complete callback
            shell.echo("");
        }
        return progress;
    }
}