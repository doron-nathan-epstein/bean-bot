const PREFIX = "!mb";

module.exports = async (client, appDAO, message) => {
  if (message.author.bot || !message.content.startsWith(PREFIX)) {
    return;
  }

  const args = message.content.slice(PREFIX.length + 1).split(/ +/);
  const commandName = args.shift().toLowerCase();
  console.log(`${message.author.tag} sent the following command: ${message}`);

  const command =
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

  if(command.adminOnly && !message.member.hasPermission("ADMINISTRATOR")) {
    return message.reply("You do not have the right permissions to execute this command!");
  }

  if (command.args && !args.length) {
    let reply = `You didn't provide any arguments, ${message.author}!`;

    if (command.usage) {
      reply += `\nThe proper usage would be: \`${PREFIX} ${command.name} ${command.usage}\``;
    }

    return message.channel.send(reply);
  }

  try {
    await command.execute(message, appDAO, args);
  } catch (error) {
    console.error(error);
    message.reply("There was an error trying to execute that command!");
  }
};
