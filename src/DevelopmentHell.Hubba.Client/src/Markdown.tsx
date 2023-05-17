import React from "react";

export namespace Markdown {
    export function parseMarkdownToHtml(markdownText: string): React.ReactElement {
        const boldRegex = /\*\*(.*?)\*\*/gm;
        const italicRegex = /\*(.*?)\*/gm;
        const headerRegex = /^(#{1,6})\s(.+)$/gm;
        const linkRegex = /\[(.+?)\]\((.+?)\)/gm;
        const codeRegex = /`(.+?)`/gm;
      
        let html = markdownText.replace(boldRegex, '<b>$1</b>');
        html = html.replace(italicRegex, '<i>$1</i>');
        html = html.replace(headerRegex, (_, hashes, text) => `<h${hashes.length}>${text}</h${hashes.length}>`);
        html = html.replace(linkRegex, '<a href="$2">$1</a>');
        html = html.replace(codeRegex, '<code>$1</code>');

        return React.createElement('div', { dangerouslySetInnerHTML: { __html: html}})
    }
}