import React from 'react';
import "./Heart.css";

interface IHeartProps {
    size: string;
    enabled: boolean;
    defaultOn: boolean;
    OnUnlike: (...args: any[]) => any;
    OnLike: (...args: any[]) => any;
}

interface IHeartState{
    size: string;
    liked: string;
    enabled: boolean;
    OnUnlike: (...args: any[]) => any;
    OnLike: (...args: any[]) => any;
}

class LikeButton extends React.Component<IHeartProps, IHeartState> {
    constructor(props: IHeartProps, enabled:boolean, ) {
        super(props);
        this.state = {
            size: props.size,
            liked: props.defaultOn ? "1" : "0",
            enabled: enabled,
            OnUnlike: props.OnUnlike,
            OnLike: props.OnLike
        }
    }

    Like() {
        this.setState({liked: "1"});
        this.state.OnLike();
    }

    Unlike() {
        this.setState({liked: "0"});
        this.state.OnUnlike();
    }

    render() {
        return (
            <div className="icon-heart">
                <label>
                    <input type="checkbox" className = "check-heart" disabled={!this.state.enabled} onChange={this.state.liked ? this.state.OnUnlike : this.state.OnLike} value={this.state.liked}>
                        <svg width="20px" height="20px" viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" className="svg-heart">
                            <path className = "heart" 
                                d="M 50,20
                                    C 30,-10, -10,30, 50,70
                                    C 110,30, 70,-10, 50,20 Z"
                                fill="gray" stroke="none" />
                        </svg>
                    </input>
                </label>
            </div>
        );
    }
}

export default LikeButton;