import terser from '@rollup/plugin-terser';

export default {
    input: './bin/nopcli.js',
    output: {
        file: './dist/nopcli.js',
        format: 'esm',
        banner: '#!/usr/bin/env node'
    },
    plugins: [
        terser({
            compress: {
                drop_console: true
            }
        })
    ]
};
