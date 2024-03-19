
import React, { useState } from 'react';
import './ChatBot.css'

const ChatBot = () => {
    const [userInput, setUserInput] = useState('');
    const [chatResponse, setChatResponse] = useState([]);

    const handleUserInput = (e) => {
        setUserInput(e.target.value);
    };

    
    const handleKeyPress = (event) => {
        // Check if the pressed key is Enter (key code 13)
        if (event.key === 'Enter') {
            handleSendMessage();
        }
    };
    async function handleSendMessage() {
        try {
            //const response = await post('ChatBot', userInput);
            const response = await fetch('http://localhost:5000/ChatBot', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                // Include any request body if needed
                body: JSON.stringify(userInput),
            });

            const data = await response.json();
            console.log(data.answer);
            //console.log(data);
            //setChatResponse(data.answer);
            setChatResponse([...chatResponse, { sender: 'user', text: userInput }, { sender: 'bot', text: data.answer }]);
            setUserInput('');
        } catch (error) {
            //console.error('Error sending message:', error);
            console.log(error);
            setChatResponse('Error communicating with the server.');
        }
    }
    
    return (
        <div className="chat-container">
            <div className="header">
                ChatBot with Custom QnA Data
            </div>
            <div className="chat-messages">
                {chatResponse.map((msg, index) => (
                    <div key={index} className={`message ${msg.sender}`}>
                        {msg.text}
                    </div>
                ))}
            </div>
            <div className="input-container">
                <input type="text" value={userInput} onChange={(e) => setUserInput(e.target.value)} className="input-field" onKeyPress={handleKeyPress} />
                <button onClick={handleSendMessage} className="send-button">Send</button>
            </div>
        </div>
    );
};

export default ChatBot;
