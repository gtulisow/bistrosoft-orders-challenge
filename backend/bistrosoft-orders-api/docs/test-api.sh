#!/bin/bash

# Script para probar la API de Bistrosoft Orders
# Uso: ./test-api.sh

BASE_URL="http://localhost:5000/api"
CONTENT_TYPE="Content-Type: application/json"

echo "🚀 Testing Bistrosoft Orders API"
echo "=================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test 1: Create Customer
echo -e "${YELLOW}📝 Test 1: Creating Customer...${NC}"
CUSTOMER_RESPONSE=$(curl -s -X POST "$BASE_URL/customers" \
  -H "$CONTENT_TYPE" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
  }')

CUSTOMER_ID=$(echo $CUSTOMER_RESPONSE | sed 's/"//g')
echo "Customer ID: $CUSTOMER_ID"
echo ""

if [ ! -z "$CUSTOMER_ID" ]; then
  echo -e "${GREEN}✅ Customer created successfully${NC}"
else
  echo -e "${RED}❌ Failed to create customer${NC}"
  exit 1
fi
echo ""

# Test 2: Get Customer
echo -e "${YELLOW}📝 Test 2: Getting Customer...${NC}"
curl -s -X GET "$BASE_URL/customers/$CUSTOMER_ID" | jq '.'
echo ""
echo -e "${GREEN}✅ Customer retrieved${NC}"
echo ""

# Test 3: Get Products to use in order (from database directly via SQL query)
echo -e "${YELLOW}📝 Test 3: You need to get Product IDs from Swagger or database${NC}"
echo "Open http://localhost:5000/swagger and execute GET /api/customers/{id}"
echo "Or check the seeded products in the database"
echo ""

# Uncomment and replace PRODUCT_ID_1 and PRODUCT_ID_2 after getting them
# echo -e "${YELLOW}📝 Test 4: Creating Order...${NC}"
# ORDER_RESPONSE=$(curl -s -X POST "$BASE_URL/orders" \
#   -H "$CONTENT_TYPE" \
#   -d "{
#     \"customerId\": \"$CUSTOMER_ID\",
#     \"items\": [
#       {
#         \"productId\": \"PRODUCT_ID_1\",
#         \"quantity\": 2
#       },
#       {
#         \"productId\": \"PRODUCT_ID_2\",
#         \"quantity\": 1
#       }
#     ]
#   }")

# ORDER_ID=$(echo $ORDER_RESPONSE | sed 's/"//g')
# echo "Order ID: $ORDER_ID"
# echo -e "${GREEN}✅ Order created${NC}"
# echo ""

# Test 5: Update Order Status
# echo -e "${YELLOW}📝 Test 5: Updating Order Status to Paid...${NC}"
# curl -s -X PUT "$BASE_URL/orders/$ORDER_ID/status" \
#   -H "$CONTENT_TYPE" \
#   -d '{
#     "newStatus": "Paid"
#   }'
# echo -e "${GREEN}✅ Order status updated${NC}"
# echo ""

# Test 6: Get Customer Orders
# echo -e "${YELLOW}📝 Test 6: Getting Customer Orders...${NC}"
# curl -s -X GET "$BASE_URL/customers/$CUSTOMER_ID/orders" | jq '.'
# echo -e "${GREEN}✅ Orders retrieved${NC}"
# echo ""

echo "=================================="
echo -e "${GREEN}🎉 Basic tests completed!${NC}"
echo ""
echo "Next steps:"
echo "1. Open Swagger: http://localhost:5000/swagger"
echo "2. Get Product IDs from the seeded data"
echo "3. Uncomment the order creation tests in this script"
echo "4. Run the script again"
