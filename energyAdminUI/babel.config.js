module.exports = {
  presets: [
    '@vue/cli-plugin-babel/preset', // vue创建生产的
    ['@babel/preset-env', { 'modules': false }]
  ],
  plugins: [
    [
      'component',
      {
        'libraryName': 'element-ui',
        'styleLibraryName': 'theme-chalk'
      }
    ]
  ]
}
