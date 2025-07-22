#!/bin/bash

# Script to push ElectronicsStore to GitHub
# Usage: ./push-to-github.sh <your-github-repo-url>

if [ $# -eq 0 ]; then
    echo "❌ Error: Please provide your GitHub repository URL"
    echo "Usage: ./push-to-github.sh <your-github-repo-url>"
    echo "Example: ./push-to-github.sh https://github.com/yourusername/ElectronicsStore.git"
    exit 1
fi

REPO_URL=$1

echo "🚀 Pushing ElectronicsStore to GitHub..."
echo "Repository URL: $REPO_URL"

# Add remote origin
echo "📡 Adding remote origin..."
git remote add origin $REPO_URL

# Push to GitHub
echo "⬆️ Pushing to GitHub..."
git push -u origin main

if [ $? -eq 0 ]; then
    echo "✅ Successfully pushed to GitHub!"
    echo "🌐 Your repository is now available at: $REPO_URL"
    echo ""
    echo "📋 Next steps:"
    echo "1. Visit your GitHub repository"
    echo "2. Check the README.md for project documentation"
    echo "3. Share the repository URL with others"
    echo ""
    echo "🤖 Chatbot features included:"
    echo "- Intelligent customer support"
    echo "- Vietnamese language processing"
    echo "- Product search capabilities"
    echo "- Hidden from admin pages"
    echo "- Real-time chat interface"
else
    echo "❌ Failed to push to GitHub"
    echo "Please check:"
    echo "1. Repository URL is correct"
    echo "2. You have access to the repository"
    echo "3. Git credentials are configured"
fi
