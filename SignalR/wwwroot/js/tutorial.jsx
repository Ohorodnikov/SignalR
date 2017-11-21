var CommentBox = React.createClass({
    render: function () {
        return (
            <div className="commentBox">
                <h1>Comments</h1>
                <CommentList />
                <CommentForm />
      </div>
        );
    }
});
var CommentList = React.createClass({
    render: function () {
        return (
            <div className="commentList">
                Hello, world! I am a CommentList.
      </div>
        );
    }
});

var CommentForm = React.createClass({
    render: function () {
        return (
            <div className="commentForm">
                Hello, world! I am a CommentForm.
      </div>
        );
    }
});
ReactDOM.render(
    <CommentBox />,
    //$("#content")
    document.getElementById('content')
);


//var CommentBox = React.createClass({
//    displayName: 'CommentBox',
//    render: function () {
//        return (
//            React.createElement('div', { className: "commentBox" },
//                "Hello, world! I am a CommentBox."
//            )
//        );
//    }
//});

//var CommentForm = React.createClass({
//    displayName: 'CommentForm',
//    render: function () {
//        return (
//            React.createElement('div', { className: "commentForm" },
//            "Comment Form")
//            );
//    }
//});



//ReactDOM.render(
//    React.createElement(CommentBox, null),
//    document.getElementById('content')
//);