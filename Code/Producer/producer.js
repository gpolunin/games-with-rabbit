const amqp = require('amqplib/callback_api');
const readline = require('readline');
const eol = require('os').EOL;
const _ = require('lodash');

const rl = readline.createInterface({
	input: process.stdin,
	output: process.stdout
});

const q = 'queue1';

var connection = null;
var channel = null;
var messageIncrement = 1;

function processQuit() {
    channel.close();
	rl.close();
	connection.close();
	process.exit(0);
}

function processEnqueue({count = 1}) {
    for (var i = 0; i < count; i++)
    {
        var message = `Messsage #${messageIncrement++}`;
        channel.sendToQueue(q, new Buffer(message));
        console.log(`${message} sent to ${q}.`);
    }
}

function processWrongCommand() {
    console.log('Wrong Command!!!')
}

function processCommand(commandRaw) {
    var splitted = commandRaw.split(' ');
    var command = splitted[0];

    splitted = splitted.slice(1, splitted.length);

    var parameters = _.chunk(splitted, 2).reduce((memo, item) => {
        memo[item[0]] = item[1].replace('"', '');
        return memo;
    }, {});

	switch (command) {
		case 'q':
			processQuit(parameters);
			break;
		case 'add':
			processEnqueue(parameters);
			break;
        case 'addQueue':
            processEnqueue(parameters);
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
    rl.question('Enter command: ', (commandRaw) => {
        console.log('You\'ve entered:', commandRaw);

        processCommand(commandRaw);
        recursiveReadLine();
    });
}

//processCommand('‌‌add count 10')

recursiveReadLine();







