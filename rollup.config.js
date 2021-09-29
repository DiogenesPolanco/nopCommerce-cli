import minify from 'rollup-plugin-minify-cli';

export default {
    input: './bin/nopcli.js',
    output: {
        file: './dist/nopcli.js'
    },
    plugins: [
        minify()
    ]
};