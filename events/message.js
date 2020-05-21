const PREFIX = "!mb";

module.exports = async (client, message) => {
  if (message.author.bot || !message.content.startsWith(PREFIX)) {
    return;
  }

  let args = message.content.slice(PREFIX.length + 1).split(/ +/);
  let commandName = args.shift().toLowerCase();
  console.log(`${message.author.tag} sent the following command: ${message}`);

  let command =
    client.commands.get(commandName) ||
    client.commands.find(
      (cmd) => cmd.aliases && cmd.aliases.includes(commandName)
    );

  if (!command) {
    return;
  }

  if (command.guildOnly && message.channel.type !== "text") {
    return message.reply("I can't execute that command inside DMs!");
  }

  if (command.args && !args.length) {
    let reply = `You didn't provide any arguments, ${message.author}!`;

    if (command.usage) {
      reply += `\nThe proper usage would be: \`${prefix} ${command.name} ${command.usage}\``;
    }

    return message.channel.send(reply);
  }

  try {
    await command.execute(message, args);
  } catch (error) {
    console.error(error);
    message.reply("There was an error trying to execute that command!");
  }
};
