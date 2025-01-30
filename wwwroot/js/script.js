const apiUrl = '/api/inventory';
const tableBody = document.querySelector('#inventoryTable tbody');
const itemForm = document.getElementById('itemForm');
const searchInput = document.getElementById('search');

// Fetch all items
async function fetchItems() {
    try {
        const response = await fetch(apiUrl);
        if (!response.ok) throw new Error('Failed to fetch items');
        const items = await response.json();
        renderTable(items);
    } catch (error) {
        console.error(error);
    }
}

// Render table rows
function renderTable(items) {
    tableBody.innerHTML = items.map(item => `
        <tr>
            <td>${item.id}</td>
            <td>${item.name}</td>
            <td>${item.description}</td>
            <td>${item.quantity}</td>
            <td>$${item.price.toFixed(2)}</td>
            <td><img src="${item.imageUrl}" alt="${item.name}" style="max-width: 100px;"></td>
            <td>
                <button onclick="editItem(${item.id})">Edit</button>
                <button onclick="deleteItem(${item.id})">Delete</button>
            </td>
        </tr>
    `).join('');
}

// Add or update item
itemForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const formData = new FormData(itemForm);
    const item = {
        name: formData.get('name'),
        description: formData.get('description'),
        quantity: parseInt(formData.get('quantity')),
        price: parseFloat(formData.get('price'))
    };

    const image = formData.get('image');

    const data = new FormData();
    data.append('item', JSON.stringify(item));
    if (image) data.append('image', image);

    const method = itemForm.dataset.id ? 'PUT' : 'POST';
    const url = itemForm.dataset.id ? `${apiUrl}/${itemForm.dataset.id}` : apiUrl;

    try {
        const response = await fetch(url, {
            method,
            body: data
        });
        if (!response.ok) throw new Error('Failed to save item');
        fetchItems();
        itemForm.reset();
        delete itemForm.dataset.id;
        itemForm.querySelector('button').textContent = 'Add Item';
    } catch (error) {
        console.error(error);
    }
});

// Search items
searchInput.addEventListener('input', async () => {
    try {
        const response = await fetch(`${apiUrl}/search?name=${searchInput.value}`);
        if (!response.ok) throw new Error('Failed to search items');
        const items = await response.json();
        renderTable(items);
    } catch (error) {
        console.error(error);
    }
});

// Edit item
async function editItem(id) {
    try {
        const response = await fetch(`${apiUrl}/${id}`);
        if (!response.ok) throw new Error('Failed to fetch item');
        const item = await response.json();
        itemForm.name.value = item.name;
        itemForm.description.value = item.description;
        itemForm.quantity.value = item.quantity;
        itemForm.price.value = item.price;
        itemForm.querySelector('button').textContent = 'Update';
        itemForm.dataset.id = id;
    } catch (error) {
        console.error(error);
    }
}

// Delete item
async function deleteItem(id) {
    try {
        const response = await fetch(`${apiUrl}/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed to delete item');
        fetchItems();
    } catch (error) {
        console.error(error);
    }
}

// Initialize
fetchItems();
