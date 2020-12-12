// postcss.config.js
module.exports = ({ env }) => ({
  plugins: [
    //require('postcss-import'),
    require('tailwindcss'),
    require('postcss-nesting'),
    require('autoprefixer'),
    env === 'production' ? require('cssnano') : false,
  ]
})