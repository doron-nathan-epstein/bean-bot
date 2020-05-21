const getUserFromMention = require("../utils/userExtensions.js");

module.exports = {
  name: "uwu",
  description: "Ask me to send an UwU to a user.",
  args: true,
  usage: "[user]",
  async execute(message, args) {
    let user = getUserFromMention(message.client, args[0]);
    if (!user) {
      return message.reply(
        "Please use a proper mention if you want to see send an UwU."
      );
    }

    message.channel.send(`${message.author}, your UwU has been sent to <@${user.id}>.`)

    return user.send(
      `<@${message.author.id}> has sent you an UwU.`
    );
  },
};
