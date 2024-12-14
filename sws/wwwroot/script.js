
const apiUrl = 'http://localhost:8080/api';

// Fetch and display documents
async function getDocuments() {
    try {
        const response = await fetch(`${apiUrl}/UploadDocument`);
        if (response.ok) {
            const documents = await response.json(); // Get the JSON response
            displayDocuments(documents); // Display documents
        } else {
            console.error('Failed to fetch documents:', response.statusText);
            alert('Failed to load documents. Please try again later.');
        }
    } catch (error) {
        console.error('Error fetching documents:', error);
        alert('An error occurred while fetching the documents.');
    }
}

// Search for documents based on a search term
async function searchDocuments() {
    event.preventDefault(); // Prevents the form from submitting and reloading the page
    try {
        let searchTerm = document.getElementById('search-query').value;
        if (searchTerm !== null && searchTerm !== "") {
            const response = await fetch(`${apiUrl}/Elasticsearch/${searchTerm}`, {
                method: 'GET',
            });

            if (!response.ok) {
                const error = await response.json();
                console.error('Search failed:', error.message, error.details);
                alert(`Search failed: ${error.message}`);
                return;
            }

            const data = await response.json();
            displayDocuments(data);
        }

    } catch (error) {
        console.error('Error searching documents:', error);
        alert('An unexpected error occurred.');
    }
}


// Display the list of documents
function displayDocuments(documents) {
    const outputDiv = document.getElementById("search-results");
    outputDiv.innerHTML = ''; // Clear existing list

    // Show a message if no documents are found
    if (documents.length === 0) {
        outputDiv.innerHTML = `<span class="error">No documents found.</span>`;
        return;
    }

    // Display each document
    documents.forEach(doc => {
        const listItem = document.createElement('div');
        listItem.classList.add('document-item');

        // Create a formatted display
        listItem.innerHTML = `
            <strong>${doc.id}</strong><br>
            <span class="file-info">Name: ${doc.name}</span>
        `;

        // Append the list item to the output
        outputDiv.appendChild(listItem);

    });
}

// Upload a document when the form is submitted

document.addEventListener('DOMContentLoaded', () => {
    const uploadForm = document.getElementById('add-document-form');
    const fileInput = document.getElementById('document-file');
    const fileName = document.getElementById('document-name');
    const responseDiv = document.getElementById('form-response');

    // Attach the submit event listener
    uploadForm.addEventListener('submit', async (event) => {
        event.preventDefault(); // Prevent default form submission
        responseDiv.innerHTML = ''; // Clear previous messages

        const file = fileInput.files[0];

        // Handle no file selection
        if (!file) {
            responseDiv.innerHTML = `<span class="error">Please select a file to upload!</span>`;
            return;
        }

        // Prepare file data for upload
        const formData = new FormData();
        formData.append('File', file);
        formData.append('Name', fileName.value);

        try {
            const response = await fetch(`${apiUrl}/UploadDocument`, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                responseDiv.innerHTML = `<span class="success">File uploaded successfully!</span>`;
                //getDocuments(); // Refresh the document list
            } else {
                responseDiv.innerHTML = `<span class="error">File upload failed. Please try again.</span>`;
            }
        } catch (error) {
            console.error('Error:', error);
            responseDiv.innerHTML = `<span class="error">An error occurred while uploading the file.</span>`;
        }
    });
    // Attach search button event
    const searchButton = document.getElementById('search-form');
    searchButton.addEventListener('submit', searchDocuments);
});