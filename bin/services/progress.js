import cliProgress from 'cli-progress'
import shell from 'shelljs';

export default class ProgressService {

    static waitProgress(type = cliProgress.Presets.shades_classic, value = 0, total = 100, ms = 10) {

        let progress = new cliProgress.Bar({
            format: 'progress [{bar}] {percentage}%'
        }, type);
        progress.start(total, 0);

        return new Promise((resolve) => {

            const timer = setInterval(() => {
                value++;
                progress.update(value)

                if (value >= progress.getTotal()) {
                    clearInterval(timer);
                    progress.stop();
                    shell.echo("");
                    resolve(this);
                }
            }, ms);
        });
    }

    static waitInfinityProgress(callback, silent=true, clearOnComplete=true, type = cliProgress.Presets.rect, value = 0, total = 100, ms = 500) {

        shell.config.silent = silent;

        const progress = new cliProgress.Bar({
            clearOnComplete: clearOnComplete,
            format: 'progress [{bar}]'
        }, cliProgress.Presets.rect);
        progress.start(total, value);

        let currentValue = value;
        let currentTotal = total;
        let timer = null;

        callback({
            progress,
            stop: () => {
                if (timer) {
                    clearInterval(timer);
                    timer = null;
                }
                progress.stop();
            },
            increment: () => {
                currentValue++;
                progress.update(currentValue);
                if (currentValue >= progress.getTotal()) {
                    currentTotal = currentTotal * 2;
                    progress.setTotal(currentTotal);
                }
            }
        });

        timer = setInterval(function () {
            currentValue++;
            progress.update(currentValue);
            if (currentValue === currentTotal) {
                currentTotal = currentTotal * 2;
                progress.setTotal(currentTotal);
            }
        }, ms);
    }

    static SetCompleted(progress, callback){
        if (typeof progress === 'object' && progress.progress) {
            progress.progress.setTotal(100);
            progress.progress.update(100);
            progress.progress.stop();
        } else if (typeof progress.stop === 'function') {
            progress.stop();
        }
        if (callback) callback();
    }
}