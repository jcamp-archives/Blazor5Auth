module.exports = {
  purge: [
    '..Client/**/*.html',
    '..Client/**/*.razor'
  ],
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {
      backgroundImage: {
        'gradient-to-b-70': "linear-gradient(to bottom, var(--tw-gradient-stops) 70%)"
      },
      colors: {
        'blazor-blue': '#052767',
        'blazor-indigo': '#3a0647',
      }
    }
  },
  variants: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/forms')
  ],
}
