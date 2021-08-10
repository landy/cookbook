﻿// Template for webpack.config.js in Fable projects
// In most cases, you'll only need to edit the CONFIG object (after dependencies)
// See below if you need better fine-tuning of Webpack options

// Dependencies. Also required: core-js, @babel/core,
// @babel/preset-env, babel-loader, sass, sass-loader, css-loader, style-loader, file-loader
var path = require("path");
var webpack = require("webpack");
var HtmlWebpackPlugin = require('html-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var MiniCssExtractPlugin = require("mini-css-extract-plugin");
const ReactRefreshWebpackPlugin = require('@pmmmwh/react-refresh-webpack-plugin');
const Dotenv = require('dotenv-webpack');
const { patchGracefulFileSystem } = require("./webpack.common.js");
patchGracefulFileSystem();

let mode = process.env.NODE_ENV
mode = mode ? mode : "production"
// If we're running the webpack-dev-server, assume we're in development mode
const isProduction = mode === 'production'
const isDevelopment = !isProduction

var CONFIG = {
    // The tags to include the generated JS and CSS will be automatically injected in the HTML template
    // See https://github.com/jantimon/html-webpack-plugin
    indexHtmlTemplate: "./src/Client/index.html",
    cssEntry: "./src/Client/style.scss",
    fsharpEntry: "./.fable-build/Application.js",
    outputDir: './deploy/public',
    assetsDir: "./src/Client/public",
    devServerPort: 8080,
    // When using webpack-dev-server, you may need to redirect some calls
    // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
    devServerProxy: {
        '/api/**': {
            target: 'http://localhost:' + (process.env.SERVER_PROXY_PORT || "5000"),
            changeOrigin: true
        },
        '/socket/**': {
            target: 'http://localhost:' + (process.env.SERVER_PROXY_PORT || "5000"),
            ws: true
        }
    },
    // Use babel-preset-env to generate JS compatible with most-used browsers.
    // More info at https://babeljs.io/docs/en/next/babel-preset-env.html
    babel: {
        plugins: [isDevelopment && require.resolve('react-refresh/babel')].filter(Boolean),
        presets: [
            ["@babel/preset-react"],
            ["@babel/preset-env", {
                "targets": "> 0.25%, not dead",
                "modules": false,
                // This adds polyfills when needed. Requires core-js dependency.
                // See https://babeljs.io/docs/en/babel-preset-env#usebuiltins
                "useBuiltIns": "usage",
                "corejs": 3
            }]
        ],
    }
}


console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve(CONFIG.indexHtmlTemplate)
    }),

    new Dotenv({
        path: "./.env",
        silent: false,
        systemvars: true
    })
];

module.exports = {
    // In development, bundle styles together with the code so they can also
    // trigger hot reloads. In production, put them in a separate CSS file.
    entry:  {
        app: [resolve(CONFIG.fsharpEntry)]
    },
    // Add a hash to the output file name in production
    // to prevent browser caching if code changes
    output: {
        path: resolve(CONFIG.outputDir),
        filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    mode: mode,
    devtool: isProduction ? "source-map" : "eval-source-map",
    optimization: {
        runtimeChunk: "single",
        moduleIds: 'deterministic',
        // Split the code coming from npm packages into a different file.
        // 3rd party dependencies change less often, let the browser cache them.
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all",
                    enforce: true
                }
            }
        },
    },
    // Besides the HtmlPlugin, we use the following plugins:
    // PRODUCTION
    //      - MiniCssExtractPlugin: Extracts CSS from bundle to a different file
    //          To minify CSS, see https://github.com/webpack-contrib/mini-css-extract-plugin#minimizing-for-production
    //      - CopyWebpackPlugin: Copies static assets to output directory
    // DEVELOPMENT
    //      - HotModuleReplacementPlugin: Enables hot reloading when code changes without refreshing
    plugins: isProduction ?
        commonPlugins.concat([
            new MiniCssExtractPlugin(),
            new CopyWebpackPlugin({
                patterns: [
                    { from: resolve(CONFIG.assetsDir) }
                ]
            }),
        ])
        : commonPlugins.concat([
            // new webpack.HotModuleReplacementPlugin(),
            new ReactRefreshWebpackPlugin()
        ]),
    resolve: {
        // // See https://github.com/fable-compiler/Fable/issues/1490
        // symlinks: false,
        modules: [resolve("./node_modules")],
        alias: {
            // Some old libraries still use an old specific version of core-js
            // Redirect the imports of these libraries to the newer core-js
            'core-js/es6': 'core-js/es'
        }
    },
    // Configuration for webpack-dev-server
    devServer: {
        publicPath: "/",
        historyApiFallback: true,
        contentBase: resolve(CONFIG.assetsDir),
        host: '0.0.0.0',
        port: CONFIG.devServerPort,
        proxy: CONFIG.devServerProxy,
        hot: true,
        inline: true
    },
    // - babel-loader: transforms JS to old syntax (compatible with old browsers)
    // - sass-loaders: transforms SASS/SCSS into JS
    // - file-loader: Moves files referenced in the code (fonts, images) into output folder
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: CONFIG.babel
                },
            },
            {
                // For pure CSS - /\.css$/i,
                // For Sass/SCSS - /\.((c|sa|sc)ss)$/i,
                // For Less - /\.((c|le)ss)$/i,
                test: /\.((c|sa|sc)ss)$/i,
                use: [
                    "style-loader",
                    {
                        loader: "css-loader",
                        options: {
                            // Run `postcss-loader` on each CSS `@import` and CSS modules/ICSS imports, do not forget that `sass-loader` compile non CSS `@import`'s into a single file
                            // If you need run `sass-loader` and `postcss-loader` on each CSS `@import` please set it to `2`
                            importLoaders: 1,
                        },
                    },
                    // {
                    //     loader: "postcss-loader",
                    //     options: { plugins: () => [postcssPresetEnv({ stage: 0 })] },
                    // },
                    // Can be `less-loader`
                    {
                        loader: "sass-loader",
                    },
                ],
            }
            // {
            //     test: /\.(sass|scss|css)$/,
            //     use: [
            //         isProduction
            //             ? MiniCssExtractPlugin.loader
            //             : 'style-loader',
            //         {
            //             loader: 'css-loader',
            //             options: {
            //                 modules: true
            //             }
            //         },
            //         {
            //             loader: 'sass-loader',
            //             options: { implementation: require("sass") }
            //         }
            //     ],
            //     include: /\.module\.(sass|scss|css)$/
            // },
            // {
            //     test: /\.(sass|scss|css)$/,
            //     use: [
            //         isProduction
            //             ? MiniCssExtractPlugin.loader
            //             : 'style-loader',
            //         {
            //             loader: 'css-loader',
            //             options: {
            //                 modules: false
            //             }
            //         },
            //         {
            //             loader: 'sass-loader',
            //             options: { implementation: require("sass") }
            //         }
            //     ],
            //     exclude: /\.module\.(sass|scss|css)$/
            // },
        ]
    }
};

function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
