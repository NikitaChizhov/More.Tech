import React from 'react';

function App() {
    return (
        <div className="login-page">
            <div className="form">
                <form className="login-form">
                    <input type="text" placeholder="username"/>
                    <input type="password" placeholder="password"/>
                    <a href="/home" className="button">login</a>
                </form>
            </div>
        </div>
    );
}

export default App;
