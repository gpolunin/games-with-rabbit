const amqp = require('amqplib/callback_api');
const readline = require('readline');
const eol = require('os').EOL;

const rl = readline.createInterface({
	input: process.stdin,
	output: process.stdout
});

const q = 'hello';

var connection = null;
var channel = null;
var messageIncrement = 1;

function processQuit() {
    channel.close();
	rl.close();
	connection.close();
	process.exit(0);
}

function processEnqueue() {
    var message = `Messsage #${messageIncrement++}`

    channel.sendToQueue(q, new Buffer(message));
console.log(`${message} sent.a`);
}

function processWrongCommand() {
    console.log('Wrong Command!!!')
}

function processCommand(command) {
	switch (command) {
		case 'q':
			processQuit();
			break;
		case 'a':
			processEnqueue();
			break;
		default:
			processWrongCommand();
	}
}

amqp.connect('amqp://192.168.99.100', (err, conn) => {
	connection = conn;
    channel = connection.createChannel((err, ch) => {
        ch.assertQueue(q, { durable: false });
    });
});

function recursiveReadLine() {
    console.log('');
    rl.question('Enter command: ', (command) => {
        console.log('You\'ve entered:', command);
        processCommand(command);
        recursiveReadLine();
    });
}

recursiveReadLine();







