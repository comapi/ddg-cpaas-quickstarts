#!/usr/bin/env node

/**
 * Module dependencies.
 */

var app = require('./app');
var fs = require('fs');
var debug = require('debug')('webhooks:server');
var http = require('http');
var https = require('https');

/**
 * Get port from environment and store in Express.
 */

var port = normalizePort(process.env.PORT || '3000');
var sslPort = normalizePort(process.env.SSL_PORT || port + 1);

/**
 * Create HTTP server with SSL support.
 */
var sslOptions = {
    key: fs.readFileSync('./ssl/key.pem'),
    cert: fs.readFileSync('./ssl/cert.pem'),
    passphrase: "password"
};

// Listen for HTTP traffic
var server = http.createServer(app).listen(port);
server.on('error', onError);
server.on('listening', onListening);

// Listen for HTTPS traffic
var sslServer = https.createServer(sslOptions, app).listen(port + 1);
sslServer.on('error', onError);
sslServer.on('listening', onListening);

/**
 * Listen on provided port, on all network interfaces.
 */
console.log('Express running...');
console.log('Browse to http://localhost:' + port);
console.log('SSL browse to https://localhost:' + sslPort);

/**
 * Normalize a port into a number, string, or false.
 */

function normalizePort(val) {
  var port = parseInt(val, 10);

  if (isNaN(port)) {
    // named pipe
    return val;
  }

  if (port >= 0) {
    // port number
    return port;
  }

  return false;
}

/**
 * Event listener for HTTP server "error" event.
 */

function onError(error) {
  if (error.syscall !== 'listen') {
    throw error;
  }

  var bind = typeof port === 'string'
    ? 'Pipe ' + port
    : 'Port ' + port;

  // handle specific listen errors with friendly messages
  switch (error.code) {
    case 'EACCES':
      console.error(bind + ' requires elevated privileges');
      process.exit(1);
      break;
    case 'EADDRINUSE':
      console.error(bind + ' is already in use');
      process.exit(1);
      break;
    default:
      throw error;
  }
}

/**
 * Event listener for HTTP server "listening" event.
 */

function onListening() {
  var addr = server.address();
  var bind = typeof addr === 'string'
    ? 'pipe ' + addr
    : 'port ' + addr.port;
  debug('Listening on ' + bind);
}
