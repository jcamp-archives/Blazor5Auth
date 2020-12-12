const path = require('path');
const gulp = require('gulp');
const rename = require("gulp-rename");
const postcss = require('gulp-postcss');

gulp.task('css', () => {
    return gulp
        .src('../Client/Styles/*.css')
        .pipe(postcss({ config: path.resolve('./') }))
        .pipe(gulp.dest('../Client/wwwroot/css/'));
});

// this purges and minifies the css files for production, but not pcss files
gulp.task('mincss', () => {
    process.env.NODE_ENV = 'production';
    return gulp
        .src('../Client/Styles/*.css')
        .pipe(postcss({ config: path.resolve('./') }))
        .pipe(rename(function (path) {
            path.extname = ".min.css";
          }))
        .pipe(gulp.dest('../Client/wwwroot/css/'));
});

gulp.task('razorcss', () => {
    return gulp
        .src('../Client/**/*.razor.pcss')
        .pipe(postcss({ config: path.resolve('./') }))
        .pipe(rename(function (path) {
            path.extname = ".css";
          }))
        .pipe(gulp.dest('../Client/'));
});

gulp.task('watch', () => {
    gulp.watch('../Client/**/*.razor.pcss', { ignoreInitial: false }, gulp.task('razorcss'));
});

gulp.task('default', gulp.series('razorcss'));
gulp.task('build', gulp.series('css', 'razorcss', 'mincss'));
