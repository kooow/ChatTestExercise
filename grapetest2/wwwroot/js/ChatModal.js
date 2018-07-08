
function ChatModal(rootElement, id, messageElements, rightDistance, sendClick, destroyEvent) {
  this.id = id;
  var main = $("<div />", { id: "chat_" + id, class: "main-section" });
  var header = this.chatHeader('Chat');
  var bodyTop = this.chatBodyTop();

  var body = '';

  for (var i = 0; i < messageElements.length; i++) {
    body += this.chatBody(messageElements[i]);
  }

  var bodyBottom = this.chatBodyBottom();

  let html_content = header + bodyTop + body + bodyBottom;
  main.html(html_content);

  $(main).css({ 'right': rightDistance + 'px' });
  $(main).toggleClass("open-more");

  var sendbutton = $(main).find(".btn-primary");
  $(sendbutton[0]).on("click", sendClick);

  var closebutton = $(main).find(".closebutton");
  $(closebutton[0]).on("click", function () {
    destroyEvent(main);
  });

  this.main = main;
  $(rootElement).append(main);

}

ChatModal.prototype.mainElement = null;
ChatModal.prototype.id = null;

ChatModal.prototype.chatHeader = function (title) {
  return `<div class="row border-chat">
        <div class="col-md-12 col-sm-12 col-xs-12 first-section">
          <div class="row">
            <div class="col-md-8 col-sm-6 col-xs-6 left-first-section">
              <p>${title}</p>
            </div>
            <div class="col-md-4 col-sm-6 col-xs-6 right-first-section">
              <a class="closebutton">X</a>
            </div>
          </div>
        </div>
      </div>`;
};

ChatModal.prototype.chatBodyTop = function () {
  return `<div class="row border-chat"><div class="col-md-12 col-sm-12 col-xs-12 second-section"><div class="chat-section"><ul>`;
};

ChatModal.prototype.chatBodyBottom = function () {
  return `</ul> </div> </div> </div>
        <div class="row border-chat">
          <div class="col-md-12 col-sm-12 col-xs-12 third-section">
            <div class="text-bar">
              <input type="text" placeholder="Write message">
                <button type="button" class="btn btn-primary btn-sm">Send</button>
            </div>
          </div>
       </div>`
};

ChatModal.prototype.chatBody = function (messageItem) {
  return `<li><div class="${(messageItem.fromcustomer ? 'left-chat' : 'right-chat')}">
                  <p>${messageItem.content}</p>
                  <span>${messageItem.created}</span>
                </div></li>`;
};

ChatModal.prototype.addNewMessage = function (messageItem) {
  var ul_element = $(this.main).find('ul');
  ul_element.append(this.chatBody(messageItem));
  var chat_section = $(this.main).find(".chat-section");
  $(chat_section).scrollTop($(chat_section).prop("scrollHeight"));
};

ChatModal.prototype.getTypedMessage = function () {
  return $(this.main).find('input').val();
};

ChatModal.prototype.clearInput = function () {
  $(this.main).find('input').val('');
};

ChatModal.prototype.destroy = function () {
  $(this.main).remove();
};