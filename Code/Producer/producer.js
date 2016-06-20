var amqp = require('amqplib/callback_api');

amqp.connect('amqp://192.168.99.100', function(err, conn) {
	conn.createChannel(function(err, ch) {
		var q = 'hello';

		ch.assertQueue(q, { durable: false });

		ch.sendToQueue(q, new Buffer('Hello world!'));
		console.log(' [x] Sent \'Hello world!\'');
	});


	setTimeout(function() { 
		conn.close(); 
		process.exit(0) 
	}, 500);
});



