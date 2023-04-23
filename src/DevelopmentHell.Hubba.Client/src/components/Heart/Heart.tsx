import React from 'react';
import "./Heart.css";

interface IHeartProps {
  size: string;
  enabled: boolean;
  defaultOn: boolean;
  OnUnlike: (...args: any[]) => any;
  OnLike: (...args: any[]) => any;
}

interface IHeartState {
  liked: boolean;
}

class LikeButton extends React.Component<IHeartProps, IHeartState> {
  constructor(props: IHeartProps) {
    super(props);
    this.state = {
      liked: props.defaultOn
    };
  }

  Like() {
    this.setState({liked: true});
    this.props.OnLike();
  }

  Unlike() {
    this.setState({liked: false});
    this.props.OnUnlike();
  }

  render() {
    return (
      <div className="icon-heart">
        <label>
          <input type="checkbox" className="check-heart" disabled={!this.props.enabled} onChange={this.state.liked ? this.props.OnUnlike : this.props.OnLike} checked={this.state.liked} />
          <svg width="20px" height="20px" viewBox="0 -20 100 100" xmlns="http://www.w3.org/2000/svg" className="svg-heart">
            <path className="heart"
              d="M 50,20
                C 30,-10, -10,30, 50,70
                C 110,30, 70,-10, 50,20 Z"
              fill="gray" stroke="none" />
          </svg>
        </label>
      </div>
    );
  }
}

export default LikeButton;
