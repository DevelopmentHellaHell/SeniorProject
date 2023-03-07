import "./WebsiteImage.css";

const WebsiteImage: React.FC<{}> = (props) => {
    return (
        <div className="website-image-container">
            <div className="address-bar">
                <div className="actions">
                    <div className="circle"></div>
                    <div className="circle"></div>
                    <div className="circle"></div>
                </div>
            </div>
            <div className="desktop"></div>
        </div>
    );
}

export default WebsiteImage;