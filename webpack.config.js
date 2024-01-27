const path = require('path');

module.exports = {  
  mode: 'development',
  entry: {
  index: './ts-out/index.js',
  },
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: 'bundle.js'
  },
};