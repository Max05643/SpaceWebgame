const path = require('path');

module.exports = {
    entry: {
        game: './Scripts/gamepage.ts',
        admin: './Scripts/admin.ts',
        settings: './Scripts/settings.ts'
    },
    mode: 'production',
    devtool: "source-map",
    optimization: {
        minimize: true,
        splitChunks: {
            chunks: 'all',
            minSize: 0,
            name : "shared"
        }
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            }
        ]
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js']
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot/js'),
        library: 'sample',
        libraryTarget: 'umd'
    }
};